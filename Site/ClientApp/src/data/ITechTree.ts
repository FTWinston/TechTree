import { ReadonlyDictionary } from './Dictionary';
import { IBuildingType } from './IBuildingType';

export interface ITechTree {
    buildings: ReadonlyDictionary<number, IBuildingType>;
}