import getopts from 'getopts';
import { getPassingPlayers, getDefensePlayers, getReceivingPlayers, getRushingPlayers } from './retrieve';
import { EMPTY, concat, from, Observable } from 'rxjs';
import { switchMap, scan, flatMap, concatAll, mergeAll } from 'rxjs/operators';
import IPlayer from './models/iplayer';
import Player from './models/player';

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

  // do this with all four types of player
  // getPassingPlayers(opts.year).subscribe();

  const observables: Observable<Player>[] = [getPassingPlayers(opts.year), getRushingPlayers(opts.year),
  getDefensePlayers(opts.year), getReceivingPlayers(opts.year)]

  const obs = from(observables).pipe(mergeAll(4));
  obs.subscribe(p => {
    x.push(p);
    console.log(x.length);
  });

  const x: IPlayer[] = [];
  const j = EMPTY.pipe(
    switchMap(() => getPassingPlayers(opts.year)),
    switchMap(() => getRushingPlayers(opts.year)),
    switchMap(() => getDefensePlayers(opts.year)),
    switchMap(() => getReceivingPlayers(opts.year))
  );
}

main();
