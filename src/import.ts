import * as fs from 'fs';
import * as path from 'path';
import getopts from 'getopts';
import { getPassingPlayers, getDefensePlayers, getReceivingPlayers, getRushingPlayers } from './retrieve';
import { from, Observable } from 'rxjs';
import { mergeAll, map } from 'rxjs/operators';
import Player from './models/player';
import { writeLine } from './helpers';

export default function getAllPlayers(year: number | string): Observable<Player[]> {
  const observables: Observable<Player[]>[] = [getPassingPlayers(year), getRushingPlayers(year),
  getDefensePlayers(year), getReceivingPlayers(year)];

  return from(observables).pipe(mergeAll(4));
}

function main(): void {
  const opts = getopts(process.argv.slice(2), {
    alias: {
      help: 'h',
      year: 'y',
    },
    default: {
      year: new Date().getFullYear() - 1
    },
  });

  writeLine(`Now retrieving all players for year ${opts.year}...`);

  let i = 0;
  const players = getAllPlayers(opts.year);
  players.pipe(
    map((ps: Player[]) => {
      const writePath = path.join(__dirname, `../temp/players-${i}.json`);
      writeLine(`Retrieved ${i + 1} of 4 OK`);
      fs.writeFileSync(writePath, JSON.stringify(ps));
      writeLine(`Wrote to ${writePath}`);
      i++;
    })
  ).subscribe();
}

main();
