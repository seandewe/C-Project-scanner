## Task 2: Shared Data Model

- Created `Shared` class library project
- Added `WordIndexEntry` class with properties:
  - FileName
  - Word
  - Count
- This model will be used for communication between agents and the master
- It will be serialized to JSON in future steps (using `System.Text.Json`)
