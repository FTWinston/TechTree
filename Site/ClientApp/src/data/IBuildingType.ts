import { IEntityType } from './IEntityType';

export interface IBuildingType extends IEntityType {
    displayRow: number;
    displayColumn: number;
    builds?: number[];
    unlocks?: number[];
    researches?: number[];
}