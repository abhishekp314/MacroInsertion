This is a simple project that I wrote in an hour to avoid adding macros in multiple/lot of .h/.cpp code while porting one of the projects. 

Inputs:
FileNamePattern - the files that it will look for. For instance if you want to add a macro ENABLE_NETWORK in all the network files that contain "Network"/"Net" in their filename.
DirectoryPath - Path of all the source files. This app will recursively go through all the files in this path.
MacroName - Macro to insert for instance "#if ENABLE_NETWORK", "#if defined(ENABLE_NETWORK)"

This looks for few patterns-
For source file, it will look for pch.h, and then insert immediately after it and adds "#endif" at the end of the file.
For header files, it will look for "#ifndef" & "#define" || "#pragma once".


Hope that helps.