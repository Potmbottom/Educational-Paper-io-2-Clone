\# Presentation Layer (Views)

\*\*Path:\*\* `Assets/\_Project/Scripts/Presentation/`



\## 1. PlayerView.cs

\*   \*\*Type:\*\* `MonoBehaviour`

\*   \*\*Purpose:\*\* Visual representation of the Player Cube.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   `\[Inject]` PlayerModel.

&nbsp;   \*   Sets MeshRenderer color.

&nbsp;   \*   \*\*Subscribes\*\* to `PlayerModel.Position` -> Updates `transform.position`.

&nbsp;   \*   \*\*Subscribes\*\* to `PlayerModel.Rotation` -> Updates `transform.rotation`.



\## 2. TerritoryView.cs

\*   \*\*Type:\*\* `MonoBehaviour` (Require: MeshFilter, MeshRenderer)

\*   \*\*Purpose:\*\* Renders the dynamic land polygon.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   `\[Inject]` PlayerModel.

&nbsp;   \*   \*\*Subscribes\*\* to `PlayerModel.OwnedTerritory`.

&nbsp;   \*   \*\*Mesh Generation:\*\* Uses `Clipper.Triangulate` to convert the Polygon Path into a Unity Mesh.

&nbsp;   \*   Updates the MeshFilter with new vertices/triangles whenever the territory changes.



\## 3. TrailView.cs

\*   \*\*Type:\*\* `MonoBehaviour` (Require: LineRenderer)

\*   \*\*Purpose:\*\* Renders the line behind the player.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   `\[Inject]` PlayerModel.

&nbsp;   \*   \*\*Subscribes\*\* to `PlayerModel.Trail.ObserveAdd` -> Adds point to LineRenderer.

&nbsp;   \*   \*\*Subscribes\*\* to `PlayerModel.Trail.ObserveReset` -> Clears LineRenderer.



\## 4. CameraFollow.cs

\*   \*\*Type:\*\* `MonoBehaviour`

\*   \*\*Purpose:\*\* Smooth camera movement.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   `\[Inject]` PlayerModel.

&nbsp;   \*   \*\*LateUpdate:\*\* Interpolates camera position towards `PlayerPosition + Offset`.

&nbsp;   \*   Looks at the player.

