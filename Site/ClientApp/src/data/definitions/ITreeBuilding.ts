import { IEntityType } from './IEntityType';
import { ITreeUnit } from './ITreeUnit';
import { IResearch } from './IResearch';

export interface ITreeBuilding extends IEntityType {
    id: number;
    row: number;
    col: number;
    unlocks: number[];
    builds: ITreeUnit[];
    researches?: IResearch[];
}