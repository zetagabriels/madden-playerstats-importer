# Madden Player Stats Importer

A way to automate the career and season stats for Madden '20.

## Retrieving

### From source

To write to a series of JSON files, use

```bash
dotnet run -p MaddenImporter/ [year]
```

JSON contains information that is not present in the Excel spreadsheets.

If you want to save the data as Excel files instead, use

```bash
dotnet run -p MaddenImporter.Excel/ [year]
```

### From dist

The executable file for your operating system can be found in the [dist/ folder](dist/). We support Windows x64, macOS x64, and Linux x64. We may have a Linux ARM build eventually for people who want to run this on a server and automate the process further.
