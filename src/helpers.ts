import * as fs from 'fs';
import { promisify } from 'util';

export const writeFileAsync = promisify(fs.writeFile);
export const existsAsync = promisify(fs.exists);

export function write(str: string): boolean {
    return process.stdout.write(str);
}

export function writeLine(line: string): boolean {
    return write(line + '\n');
}

export function generateUrl(year: number | string, type: 'passing' | 'rushing' | 'receiving' | 'defense'): string {
    return `https://www.pro-football-reference.com/years/${year}/${type}.htm`;
}

export async function checkOrCreatePath(path: string): Promise<void> {
    if (!await existsAsync(path)) {
        await promisify(fs.mkdir)(path);
        writeLine(`Created directory ${path}.`);
    }
}
