import getopts from 'getopts';
import getAllPlayers from './import';

// import, then export. do not save files to disk on import
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

  const players = getAllPlayers(opts.year);
}
