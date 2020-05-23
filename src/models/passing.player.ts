import Player from './player';

export default class PassingPlayer extends Player {
  completions!: number;
  attemptedPasses!: number;
  passingYards!: number;
  passingTouchdowns!: number;
  interceptions!: number;
  firstDowns!: number;
  longestPass!: number;
  sacksTaken!: number;
  fourthQuarterComebacks!: number;

  convert(json: any): void {
    super.convert(json);
    this.completions = json.cmp || 0;
    this.attemptedPasses = json.pass_att || 0;
    this.passingYards = json.pass_yds || 0;
    this.passingTouchdowns = json.pass_td || 0;
    this.interceptions = json.pass_int || 0;
    this.firstDowns = json.pass_first_down || 0;
    this.longestPass = json.pass_long || 0;
    this.sacksTaken = json.pass_sacked || 0;
    this.fourthQuarterComebacks = json.comebacks || 0;
  }
}
