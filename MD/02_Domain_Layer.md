\# Domain Layer

\*\*Path:\*\* `Assets/\_Project/Scripts/Domain/`



\## 1. ReactiveModel.cs

\*   \*\*Type:\*\* Abstract Base Class

\*   \*\*Purpose:\*\* Provides a standard way to handle disposal of reactive subscriptions.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   Holds a `CompositeDisposable`.

&nbsp;   \*   Implements `IDisposable`.



\## 2. LevelModel.cs

\*   \*\*Type:\*\* Pure C# Class

\*   \*\*Purpose:\*\* Static data about the map.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   Defines map `Bounds` (float) for the clamping logic.



\## 3. PlayerModel.cs

\*   \*\*Type:\*\* `ReactiveModel`

\*   \*\*Purpose:\*\* The source of truth for the Player.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   \*\*ReactiveProperty Position:\*\* Current Vector3.

&nbsp;   \*   \*\*ReactiveProperty Rotation:\*\* Current Quaternion.

&nbsp;   \*   \*\*ReactiveCollection Trail:\*\* List of Vector3 points representing the tail.

&nbsp;   \*   \*\*ReactiveProperty OwnedTerritory:\*\* Stores `Clipper2Lib.PathsD` (the polygon shape).

&nbsp;   \*   \*\*Config:\*\* Speed, TrailSpawnDistance, PlayerColor.

&nbsp;   \*   \*\*Init:\*\* Creates the initial small starting square polygon.

