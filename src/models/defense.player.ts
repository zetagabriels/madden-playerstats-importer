import Player from './player';

export default class DefensePlayer extends Player {
  interceptions!: number;
  interceptionYards!: number;
  interceptionTouchdowns!: number;
  longestInterceptionReturn!: number;
  passesDefended!: number;
  forcedFumbles!: number;
  fumblesRecovered!: number;
  fumbleYards!: number;
  fumbleTouchdowns!: number;
  sacks!: number;
  assistedTackles!: number;
  tacklesForLoss!: number;
  safety!: number;

  convert(json: any): void {
    super.convert(json);
    this.interceptions = json.def_int || 0;
    this.interceptionYards = json.def_int_yds || 0;
    this.interceptionTouchdowns = json.def_int_td || 0;
    this.longestInterceptionReturn = json.def_int_long || 0;
    this.passesDefended = json.pass_defended || 0;
    this.forcedFumbles = json.fumbles_forced || 0;
    this.fumblesRecovered = json.fumbles_rec || 0;
    this.fumbleYards = json.fumbles_rec_yds || 0;
    this.fumbleTouchdowns = json.fumbles_rec_td || 0;
    this.sacks = json.sacks || 0;
    this.assistedTackles = json.tackles_assists || 0;
    this.tacklesForLoss = json.tackles_loss || 0;
    this.safety = json.safety_md || 0;
  }
}
