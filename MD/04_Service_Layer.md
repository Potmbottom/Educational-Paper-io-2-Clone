\# Service Layer

\*\*Path:\*\* `Assets/\_Project/Scripts/Service/`



\## 1. IInputProvider.cs

\*   \*\*Type:\*\* Interface

\*   \*\*Purpose:\*\* Abstraction for input to allow switching between Keyboard/Touch/AI.

\*   \*\*Contract:\*\* `Vector3 GetDirection();`



\## 2. KeyboardInputProvider.cs

\*   \*\*Type:\*\* Class (Implements IInputProvider)

\*   \*\*Purpose:\*\* Unity Legacy Input Wrapper.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   Reads Horizontal/Vertical axis.

&nbsp;   \*   Returns normalized direction vector.



\## 3. TerritoryCalculator.cs

\*   \*\*Type:\*\* Pure C# Class

\*   \*\*Purpose:\*\* Complex Geometry Math wrapper for Clipper2.

\*   \*\*Responsibilities:\*\*

&nbsp;   \*   `IsPointInTerritory()`: Checks if a Vector3 is inside the current polygon.

&nbsp;   \*   `CalculateExpansion()`:

&nbsp;       \*   Takes current territory and the new trail.

&nbsp;       \*   Performs \*\*Union\*\* (adds trail to land).

&nbsp;       \*   Performs \*\*Morphological Closing\*\* (Dilate then Erode) to seal gaps.

&nbsp;       \*   Removes Holes (Inner polygons).

&nbsp;       \*   Returns the new `PathsD`.

