import getopts from 'getopts';
import { generateUrl } from './helpers';
import getRemoteData from './retrieve';
import PassingPlayer from './models/passing.player';

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
  const url = generateUrl(opts.year, 'passing');
  getRemoteData<PassingPlayer>(url).subscribe();

  /* EMPTY.pipe(
    switchMap(() => {
      const url = generateUrl(opts.year, 'rushing');
      console.log('ok');
      return getRemoteData<RushingPlayer>(url);
    })
  ).subscribe(); */
}

main();
