import React, { useState, useEffect } from 'react';
import './App.css';
import { IGameDefinition } from './data/definitions/IGameDefinition';
import { GameOverview } from './components/GameOverview';

const App: React.FC = () => {
    const [game, setGame] = useState<IGameDefinition>();

    useEffect(() => {
        loadGameDefinition(setGame);
    }, []);

    const gameDisplay = game
        ? <GameOverview data={game} />
        : undefined

    return (
        <div className="App">
            <button onClick={() => loadGameDefinition(setGame)}>load new game</button>
            {gameDisplay}
        </div>
    );
}

export default App;

async function loadGameDefinition(
    setGame: (data: IGameDefinition) => void
) {
    const response = await fetch('/Game/Generate');
    const data = await response.json() as IGameDefinition;
    console.log('received definition', data);
    setGame(data);
}