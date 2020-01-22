import React, { FunctionComponent, useMemo } from 'react';
import { ITechTree } from '../../data/definitions/ITechTree';
import { TreeBuilding, Connection } from './TreeBuilding';
import './TechTree.css';
import { IBuildingType } from '../../data/definitions/IBuildingType';
import { ReadonlyDictionary } from '../../data/Dictionary';

interface Props {
    data: ITechTree;
}

export const TreeContext = React.createContext<ITechTree>({
    buildings: {},
    units: {},
});

export const TechTree: FunctionComponent<Props> = props => {
    const buildings = useMemo(
        () => {
            const relatedConnections = populateIncomingConnections(props.data.buildings)

            const buildingDisplays: JSX.Element[] = [];

            for (const id in props.data.buildings) {
                const building = props.data.buildings[id]!;

                const incomingConnections = determineIncomingConnections(building, relatedConnections);

                const outgoingConnections = unlocksAnyBuildings(building.unlocks, props.data.buildings)
                    ? Connection.Middle
                    : Connection.None;

                buildingDisplays.push(
                    <TreeBuilding
                        key={id}
                        id={id as unknown as number}
                        incomingConnections={incomingConnections}
                        outgoingConnections={outgoingConnections}
                    />
                )
            }

            return buildingDisplays;
        }
        , [props.data.buildings]
    )

    return (
        <TreeContext.Provider value={props.data}>
            <div className="techTree">
                {buildings}
            </div>
        </TreeContext.Provider>
    );
}

function populateIncomingConnections(buildings: ReadonlyDictionary<number, IBuildingType>) {
    const relatedConnections = new Map<IBuildingType, IBuildingType[]>();   

    // for each building, link its parent and ITS SIBLINGS, that is other buildings unlocked by the same parent.
    for (const parentID in buildings) {
        const parent = buildings[parentID]!;

        if (parent.unlocks === undefined) {
            continue;
        }

        for (const childID of parent.unlocks) {
            const child = buildings[childID];
            if (child === undefined) {
                continue;
            }

            let childConnections = relatedConnections.get(child);
            if (childConnections === undefined) {
                childConnections = [];
                relatedConnections.set(child, childConnections);
            }

            childConnections.push(parent);

            for (const siblingID of parent.unlocks) {
                if (siblingID === childID) {
                    continue;
                }

                    
                const sibling = buildings[siblingID];
                if (sibling === undefined) {
                    continue;
                }

                childConnections.push(sibling);
            }
        }
    }

    return relatedConnections;
}

function determineIncomingConnections(building: IBuildingType, relatedConnections: Map<IBuildingType, IBuildingType[]>) {
    let connections = Connection.None;

    const relations = relatedConnections.get(building);
    if (relations !== undefined) {
        for (const relation of relations) {
            if (relation === building) {
                continue; // TODO: ensure we don't need this continue
            }

            if (relation.displayColumn < building.displayColumn) {
                connections |= Connection.Left;
            }
            else if (relation.displayColumn > building.displayColumn) {
                connections |= Connection.Right;
            }
            else {
                connections |= Connection.Middle;
            }
        }
    }

    return connections;
}

function unlocksAnyBuildings(ids: undefined | number[], allBuildings: ReadonlyDictionary<number, IBuildingType>) {
    if (ids === undefined) {
        return false;
    }

    for (const id of ids) {
        if (allBuildings[id] !== undefined) {
            return true;
        }
    }

    return false;
}