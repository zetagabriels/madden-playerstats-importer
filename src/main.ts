import getopts from 'getopts';
import { generateUrl } from './helpers';
import getRemoteData from './retrieve';

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

    const url = generateUrl(opts.year, 'passing');
    getRemoteData(url).subscribe();
}

main();