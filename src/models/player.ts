export default class Player {
  name!: string;
  team!: string;
  position!: string;
  gamesPlayed!: number;
  gamesStarted!: number;

  constructor() {
    this.name = 'cool guy';
  }

  public static convert(json: any): Player {
    const p = new Player();
    p.name = json.player;
    p.team = json.team;
    p.position = json.pos;
    p.gamesPlayed = json.g;
    p.gamesStarted = json.gs;
    return p;
  }
}