import * as fs from 'fs';
import * as path from 'path';
import getopts from 'getopts';
import { getPassingPlayers, getDefensePlayers, getReceivingPlayers, getRushingPlayers } from './retrieve';
import { EMPTY, from, Observable } from 'rxjs';
import { switchMap, mergeAll, map } from 'rxjs/operators';
import Player from './models/player';

export function getAllPlayers(year: number | string): Observable<Player[]> {
  // do this with all four types of player
  // getPassingPlayers(opts.year).subscribe();

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

  let i = 0;
  const players = getAllPlayers(opts.year);
  players.pipe(
    map((ps: Player[]) => {
      fs.writeFileSync(path.join(__dirname, `../temp/players-${i}.json`), JSON.stringify(ps));
      i++;
    })
  ).subscribe();
}

main();
