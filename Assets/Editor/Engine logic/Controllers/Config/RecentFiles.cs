using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class RecentFiles {

    public const int OPENED_TODAY = 0;

    public const int OPENED_YESTERDAY = 1;

    public const int OPENED_THIS_WEEK = 2;

    public const int OPENED_THIS_MONTH = 3;

    public const int MAX_FILES = 10;

    private RecentFile[] recentFiles;

    private int nFiles;

    public RecentFiles(Properties properties)
    {

        nFiles = 0;
        if (properties.getProperty("RecentFiles")!=null)
        {
            try
            {
                nFiles = int.Parse(properties.getProperty("RecentFiles"));
            }
            catch (Exception e)
            {
                // If any problem reading or parsing the number of recent files, do not use that field
            }
        }
        recentFiles = new RecentFile[MAX_FILES];
        int nCorrectFiles = nFiles;
        for (int i = 0; i < nFiles; i++)
        {
            string pathKey = "RecentFile." + i + ".FilePath";
            string path = properties.getProperty(pathKey);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                string dateKey = "RecentFile." + i + ".DateOpened";
                string date = properties.getProperty(dateKey);
             
                    RecentFile recentFile = new RecentFile(path, date);
                    recentFiles[i] = recentFile;
            }
            else {
                recentFiles[i] = null;
                nCorrectFiles--;
            }

        }

        RecentFile[] temp = new RecentFile[MAX_FILES];
        for (int i = 0, j = 0; i < nFiles; i++)
        {
            if (recentFiles[i] != null)
            {
                temp[j] = recentFiles[i];
                j++;
            }
        }
        nFiles = nCorrectFiles;
        recentFiles = temp;

        orderFilesByDate();
    }

    public void fillProperties(Properties properties)
    {

        int i = 0;
        foreach (RecentFile file in recentFiles)
        {
            if (i >= nFiles)
                break;
            string pathKey = "RecentFile." + i + ".FilePath";
            string path = file.getAbsolutePath();
            properties.setProperty(pathKey, path);
            string dateKey = "RecentFile." + i + ".DateOpened";
            string date = file.ToString();
            properties.setProperty(dateKey, date);
            i++;
        }
        string nFilesKey = "RecentFiles";
        string nFilesS = this.nFiles.ToString();
        properties.setProperty(nFilesKey, nFilesS);

    }

    public string[][] getRecentFilesInfo(int r)
    {
        List<RecentFile> toReturn = new List<RecentFile>();
        //Date today = new Date();
        DateTime right = new DateTime();
        right.AddDays(-r);
        //right.set(Calendar.HOUR_OF_DAY, 0);
        int n = 0;
        foreach (RecentFile file in this.recentFiles)
        {
            if (n >= nFiles)
                break;
            DateTime actual = new DateTime();
            actual = file.getDate();
            if (actual.CompareTo(right.Date) <= 0)
            {
                toReturn.Add(file);
            }
            n++;
        }

        string[][] info = new string[toReturn.Count][];
        for (int i = 0; i < toReturn.Count; i++)
        {
            info[i]=new string[3];
        }
        for (int i = 0; i < toReturn.Count; i++)
        {
            info[i][0] = toReturn[i].getAbsolutePath();
            info[i][1] = toReturn[i].getDate().Date.ToString();
            info[i][2] = toReturn[i].getDate().TimeOfDay.ToString();
        }
        return info;
    }

    public string[][] getRecentFilesInfo(int l, int r)
    {

        List<RecentFile> toReturn = new List<RecentFile>();
        //Date today = new Date();
        DateTime left = new DateTime();
        left.AddDays(-1);
        //left.set(Calendar.HOUR_OF_DAY, 0);
        DateTime right = new DateTime();
        right.AddDays(-r);
        //right.set(Calendar.HOUR_OF_DAY, 0);
        int n = 0;
        foreach (RecentFile file in this.recentFiles)
        {
            if (n >= nFiles)
                break;

            DateTime actual = new DateTime();
            actual = file.getDate();
            if (actual.Date.CompareTo(right.Date) <= 0 && actual.Date.CompareTo(left.Date) > 0)
            {
                toReturn.Add(file);
            }
            n++;
        }

        string[][] info = new string[toReturn.Count][];
        for (int i = 0; i < toReturn.Count; i++)
        {
            info[i] = new string[3];
        }

        for (int i = 0; i < toReturn.Count; i++)
        {
            info[i][0] = toReturn[i].getAbsolutePath();
            info[i][1] = toReturn[i].getDate().Date.ToString();
            info[i][2] = toReturn[i].getDate().TimeOfDay.ToString();
        }
        return info;
    }

    public void orderFilesByDate()
    {

        for (int i = 0; i < nFiles; i++)
        {
            RecentFile minDate = recentFiles[i];
            int minPos = i;
            //Seek the min value
            for (int j = i + 1; j < nFiles; j++)
            {
                if (recentFiles[j].getDate().Date < minDate.getDate().Date)
                {
                    minDate = recentFiles[j];
                    minPos = j;
                }
            }
            //Swap min value, locating it in pos. i: i<->minPos
            recentFiles[minPos] = recentFiles[i];
            recentFiles[i] = minDate;
        }
    }

    public void fileLoaded(string path)
    {

        //Browse the array to find occurrences of the file
        bool inserted = false;
        for (int i = 0; i < nFiles; i++)
        {
            RecentFile file = recentFiles[i];

            //If the file exists in the array (previously opened), update its date
            if (file.getAbsolutePath().Equals(path))
            {
                file.setDate(new DateTime());
                inserted = true;
            }
        }

        //If file was not found, insert it into the array, removing the "oldest" if there's no room enough
        if (!inserted)
        {
            RecentFile newFile = new RecentFile(path);

            //If there is no room
            if (nFiles == recentFiles.Length)
            {
                //Remove the oldest. As it is ordered, it is the first one. Replace it by the new one
                recentFiles[0] = newFile;
            }
            //In case there is room enough, just insert it in the las position
            else {
                recentFiles[nFiles] = newFile;
                nFiles++;
            }
        }

        //Finally, order the array
        this.orderFilesByDate();
    }
}
