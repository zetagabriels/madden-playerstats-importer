import getopts from 'getopts';
import Player from './models/player';

export default function exportPlayersToFile(players: Player[], filePath: string): void {
  // from players array, export to filePath as XLSX(?)
}

function main(): void {
  // read players JSON or CSV file
  const opts = getopts(process.argv.slice(2), {
    alias: {
      help: 'h',
      format: 'f',
    },
    default: {
      format: 'json'
    },
  });


}
