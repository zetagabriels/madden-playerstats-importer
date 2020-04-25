export function write(str: string) {
    process.stdout.write(str);
}

export function writeLine(line: string) {
    return write(line + '\n');
}