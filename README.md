# Madden Player Stats Importer

hey football nerds here's some code for you

.exe and .sh versions coming soon but for now you'll need node

## Importing

To import:

```bash
npm run import
```

### Import options

You will need to add `--` after the `npm run import` statement.

```bash
 -y, --year     the year to retrieve data from
 -p, --path     the path to save files to (absolute or relative)
```

#### Example

To retrieve all player stats from the year 2015:

```bash
npm run import -- -y 2015
```

## Exporting

To export:

```bash
npm run export
```

### Export options

You will need to add `--` after the `npm run export` statement.

```bash
 -f, --format   the format of the file that contains the season stats
```
