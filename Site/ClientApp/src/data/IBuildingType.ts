import { IPurchasable } from './IPurchaseable';

export interface IBuildingType extends IPurchasable {
    displayRow: number;
    displayColumn: number;
    builds?: number[];
    unlocks?: number[];
    researches?: number[];
}