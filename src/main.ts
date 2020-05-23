import * as path from 'path';
import getopts from 'getopts';
import getAllPlayers from './import';

// import, then export. do not save files to disk on import
function main(): void {
  const opts = getopts(process.argv.slice(2), {
    alias: {
      help: 'h',
      year: 'y',
      format: 'f',
      path: 'p',
    },
    default: {
      year: new Date().getFullYear() - 1,
      format: 'json',
      path: path.join(__dirname, '../temp/')
    },
  });

  const players = getAllPlayers(opts.year);
}

main();
