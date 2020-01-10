import { ReadonlyDictionary } from './Dictionary';
import { IBuildingType } from './IBuildingType';
import { IUnitType } from './IUnitType';

export interface ITechTree {
    buildings: ReadonlyDictionary<number, IBuildingType>;
    units: ReadonlyDictionary<number, IUnitType>;
}