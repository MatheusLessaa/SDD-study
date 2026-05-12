# /create-task

Objective:
Create a new improvement task in an existing SDD project.

Required flow:
1. Read the existing specs.
2. Identify the impact on:
   - main spec
   - tasks
   - acceptance criteria
   - technical plan
   - tests
3. Propose the change without implementing code yet.
4. Generate a documentation diff.
5. Ask the user for confirmation.
6. Only after approval, update the files.
7. Create an execution checklist.

Flow:
User request
   ↓
/create-task
   ↓
AI analyzes the current specs
   ↓
AI proposes the task + documentation impact
   ↓
You validate
   ↓
AI updates specs/tasks
   ↓
Only then comes implementation
