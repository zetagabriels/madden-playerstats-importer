export function write(str: string): boolean {
    return process.stdout.write(str);
}

export function writeLine(line: string): boolean {
    return write(line + '\n');
}

export function generateUrl(year: number | string, type: 'passing' | 'rushing' | 'defense'): string {
    return `https://www.pro-football-reference.com/years/${year}/${type}.htm`;
}