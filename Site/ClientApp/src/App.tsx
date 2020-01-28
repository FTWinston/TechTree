import React, { useState, useEffect } from 'react';
import './App.css';
import { TechTree } from './components/tree';
import { ITechTree } from './data/definitions/ITechTree';
import { IBattlefield } from './data/instances/IBattlefield';
import { Battlefield } from './components/Battlefield';
import { IGameDefinition } from './data/definitions/IGameDefinition';

const App: React.FC = () => {
    const [treeData, setTreeData] = useState<ITechTree>();
    const [battlefield, setBattlefield] = useState<IBattlefield>();

    useEffect(() => {
        loadGameDefinition(setTreeData, setBattlefield);
    }, []);

    const treeDisplay = treeData
        ? <TechTree data={treeData} />
        : undefined

    const battlefieldDisplay = battlefield
        ? <Battlefield data={battlefield} />
        : <div>please wait</div>;

    return (
        <div className="App">
            {battlefieldDisplay}
            <button onClick={() => loadGameDefinition(setTreeData, setBattlefield)}>new game</button>
            {treeDisplay}
        </div>
    );
}

export default App;

async function loadGameDefinition(
    setTree: (data: ITechTree) => void,
    setBattlefield: (data: IBattlefield) => void
) {
    const response = await fetch('/Game/Generate');
    const data = await response.json() as IGameDefinition;
    console.log('received definition', data);
    setTree(data.techTree);
    setBattlefield(data.battlefield);
}