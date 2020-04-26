export default class Player {
  name!: string;
  team!: string;
  position!: string;
  gamesPlayed!: number;
  gamesStarted!: number;

  constructor(json: any) {
    this.name = json.player;
    this.team = json.team;
    this.position = json.pos;
    this.gamesPlayed = json.g;
    this.gamesStarted = json.gs;
  }
}