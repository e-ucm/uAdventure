using UnityEngine;
using System.Collections;

public class DescriptionController{

    private Description description;

    private ConditionsController conditionsController;

    public DescriptionController(Description description)
    {
        this.description = description;

        if (description.getConditions() == null)
        {
            description.setConditions(new Conditions());
        }


        conditionsController = new ConditionsController(description.getConditions());

    }

    public ConditionsController getConditionsController()
    {
        return conditionsController;
    }

    public string getName()
    {
        return description.getName();

    }

    public string getBriefDescription()
    {
        return description.getDescription();

    }

    public string getDetailedDescription()
    {
        return description.getDetailedDescription();

    }

    public string getNameSoundPath()
    {
        return description.getNameSoundPath();

    }

    public string getDescriptionSoundPath()
    {
        return description.getDescriptionSoundPath();

    }

    public string getDetailedDescriptionSoundPath()
    {
        return description.getDetailedDescriptionSoundPath();

    }


    public void setDescriptionData(Description description)
    {
        this.description = description;
    }


    public Description getDescriptionData()
    {
        return description;
    }
}
