import React, { useState, useEffect } from 'react';
import './App.css';
import { TechTree } from './components/tree';
import { ITechTree } from './data/definitions/ITechTree';
import { IBattlefield } from './data/instances/IBattlefield';

const App: React.FC = () => {
    const [treeData, setTreeData] = useState<ITechTree>();
    const [battlefield, setBattlefield] = useState<IBattlefield>();

    useEffect(() => { loadData(setTreeData, setBattlefield) }, []);

    const treeOrMessage = treeData === undefined
        ? <div>please wait</div>
        : <TechTree data={treeData} />

    return (
        <div className="App" onClick={() => loadData(setTreeData, setBattlefield)}>
            {treeOrMessage}
        </div>
    );
}

export default App;

async function loadData(
    setTree: (data: ITechTree) => void,
    setBattlfield: (data: IBattlefield) => void
) {
    {
        const response = await fetch('/Game/Tree');
        const data = await response.json() as ITechTree;
        console.log('received tree', data);
        setTree(data);
    }

    {
        const response = await fetch('/Game/Battlefield');
        const data = await response.json() as IBattlefield;
        console.log('received battlefield', data);
        setBattlfield(data);
    }
}