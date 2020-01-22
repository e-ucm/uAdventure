#!/usr/bin/python
#
# chkheader.py: a script to ensure source-files share a common header
# 
# Manuel Freire (manuel.freire@fdi.ucm.es)
#
import os, sys

def chk_header(target, headerLines):
    needsHeaders = False
    targetLines = [];
    with open(target, "r") as targetFile:
        targetLines = targetFile.readlines()
        if len(targetLines) > 1 and not targetLines[1].endswith(headerLines[0]):
            print 'Adding header to `' + target + '`'
            needsHeaders = True
    if needsHeaders:
        if target.endswith('.cs'): 
            with open(target, "w") as targetFile:
                targetFile.write('/*\n');
                for headerLine in headerLines:
                    targetFile.write(' * ' + headerLine);
                targetFile.write('\n */\n');
                targetFile.writelines(targetLines)          
        else:
            print 'ERROR: do not know how to add headers to ' + target

def visitor(headerLines, dirName, names):
    print 'Visiting ' + dirName
    for name in names:
        fullName = os.path.join(dirName, name)
        print 'Checking ' + fullName + ' - isfile? ' + str(os.path.isfile(fullName))
        if os.path.isfile(fullName) and name.endswith('.cs'):
            chk_header(fullName, headerLines)

if __name__ == '__main__':
    currentDir = sys.argv[1] if len(sys.argv) > 1 else "."
    header = sys.argv[2] if len(sys.argv) > 2 else "HEADER"
    with open(header, "r") as headerFile:
        headerLines = headerFile.readlines()
    os.path.walk(currentDir, visitor, headerLines)