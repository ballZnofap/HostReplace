#Host File Replacer

This is a tool used to lock your host files to block unwanted sites.
The solution contains to projects, each would monitor each other.

It's designed to ensure the entries in the host file cannot be modified/deleted, but can be added to.

Be careful before you install this tool.

##Project Description
There are two projects in this solution
- HostFileReplace
- Monitor

###HostFileReplace
This is where the main work takes place, it overwrites the existing host file every once in a while, and keeps the Host file open to prevent any manual editing
And if there is anything to be added, it will read the addition from the addendum file "hosts_addendum.txt" and incorporate it to the new source.

##Monitor
Monitor makes sure HostFileReplace is running, and restarts it as soon as it's stopped.

##How to install
Build both HostFileReplace and Monitor in release mode and copy the executables to a folder

Install the services by opening command prompt as administrator and execute the following commands

cd {folderpath}
HostFileReplace.exe install
HostFileReplaceMonitor.exe install

For everyone's convenience, an Install.bat file is contained under the root, running it in administrator mode will copy the necessary files under "output" folder
and install the services from the "output" folder (meaning you will have trouble deleting this folder)

If that is too hard, an "output" folder is already

##How to Use
Open notepad (or another text editor) in administrator mode
Create a file C:\\Windows\\System32\\drivers\\etc\\drivers\\etc\\hosts_addendum.txt
Find a host file you would like to use on the Internet and copy the contents to hosts_addendum.txt
(For example, http://someonewhocares.org/hosts/)

Save the file, in a few seconds, you should see hosts_addendum deleted, and both hosts and hosts_src file will now contain the contents added

Good luck

##Potential Future
This is a very simple tool I created a while ago to fix my Internet addiction. 

If there is enough interest, I may take on some possible addition

Possible addition include:
- Create commandline arguments
- Use GUI to hide the intricacies of host file
- Download updates to host file easily