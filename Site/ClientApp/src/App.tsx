import React, { useState, useEffect } from 'react';
import './App.css';
import { ITechTree } from './data/ITechTree';
import { TechTree } from './components/TechTree';

const App: React.FC = () => {
    const [data, setData] = useState<ITechTree>();

    useEffect(() => { loadData(setData) }, []);

    const treeOrMessage = data === undefined
        ? <div>please wait</div>
        : <TechTree data={data} />

    return (
        <div className="App" onClick={() => loadData(setData)}>
            {treeOrMessage}
        </div>
    );
}

export default App;

async function loadData(setData: (data: ITechTree) => void) {
    const response = await fetch('/Game/Tree');
    const treeData = await response.json() as ITechTree;
    console.log('recieved tree', treeData);
    setData(treeData);
}