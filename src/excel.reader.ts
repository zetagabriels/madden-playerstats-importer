import * as path from 'path';
import * as xlsx from 'xlsx';

type filePosition = 'defense' | 'kick_punt_return' | 'kicking' | 'passing' | 'receiving' | 'rushing';

// https://github.com/SheetJS/sheetjs

export default class ExcelReader {
  private static csvPath = '../excel';

  private static getSeasonStats(year: string | number): string {
    return path.join(__dirname, this.csvPath, `${year}_SEASON_STATS_ALL.xlsx`);
  }

  private static getCareerStats(): string {
    return path.join(__dirname, this.csvPath, `CAREER_STATS_ALL.xlsx`);
  }
}
