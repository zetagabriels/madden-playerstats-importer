# Madden Player Stats Importer

A way to automate the career and season stats for Madden '20.

## Retrieving

### From source

If you want to save the data as an Excel file, use

```bash
dotnet run -p MaddenImporter.Excel/ [year]
```

If you wish to use a different file format, the `MaddenImporter` class library contains all the necessary functions and data.

### From dist

The executable file for your operating system can be found in the `dist/` folder. We support Windows x64, macOS x64, Linux x64, and Linux ARM.
