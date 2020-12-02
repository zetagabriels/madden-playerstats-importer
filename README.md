# Madden Player Stats Importer

A way to automate the career and season stats for Madden '20.

## Retrieving

### Login

For career stats, this program pulls from [StatHead](https://stathead.com), and it requires a paid subscription to access all datarows. You can provide your username and password in two ways:

1. Use the `-u (username)` and `-p (password)` command-line arguments while running the program; or
2. Create a file in the root directory called `login.private` with two lines. The first line is your username, and the second line is your password.

**The program will not save your username and / or password. You must either create a file or enter the information every time you run the program.**

### From source

If you want to save the data as an Excel file, use

```bash
dotnet run -p MaddenImporter.Excel/ -- [-y year] [--path path] [--career] [-u username] [-p password]
```

If you wish to use a different file format, the `MaddenImporter` class library contains all the necessary functions and data.

### From dist

The executable file for your operating system can be found in the `dist/` folder, and in the Releases page. We support Windows x64, macOS x64, Linux x64, and Linux ARM. If you are using an ARM-based machine, you may need to build the geckodriver executable from the source code.

```bash
./maddenimporter.excel(.exe) [-y year] [--path path] [--career] [-u username] [-p password]
```
