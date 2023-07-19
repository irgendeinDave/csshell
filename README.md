# Csshell
A shell made for Linux machines with C#. It might work on other operatig systems but is mainly made for Linux. Many features are still under development.

## Features
Csshell supports running scripts, reading and writing environment variables and has some basic autocompletion.
### History
Previous commands are saved in ~/.csshellhist and can be accessed by typing `!!`. You can give an index after the `!!` to go back to older entries in the historyfile.

### Scripts
To write a script create a file with any ending you want and start it with `#!/the/path/to/your/executable/csshell`.
Make it executable and run it in any shell. All the lines will be executed one after another in a new instance of csshell. When the script ends this instance will end automatically

### Variables
To create an environment variable use the `set`-keyword followed by the name, an equal sign and the value without any spaces in between. Examle: `set test=testvalue`.
To read this value use a dollar sign followed by the name of the variable.

### Exit Codes
Exit codes can be saved in the `?` environment variable.

## Usage 
Clone the repository and compile it using the `dotnet build` command. The shell can be started by running it in another shell or by setting it as default in your terminal emulator.
