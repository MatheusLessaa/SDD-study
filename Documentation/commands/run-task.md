# /run-task

Objective:
Run an existing task from the project task list after confirming the selected task with the user.

Expected input:

```text
/run-task <task-number>
```

Example:

```text
/run-task 7.19
```

Required flow:
1. Read the project task list in `Documentation/tasks/task.md`.
2. Find the task whose heading matches the provided task number.
   - Example: `/run-task 7.19` must match `Task 7.19`.
3. If no task number is provided, stop and ask the user to provide one.
4. If the task number is not found, stop and inform the user that the task was not found in the task list.
5. Return a brief explanation of the matched task.
   - The explanation must follow the task title.
   - The explanation may summarize the acceptance criteria.
   - The explanation must not invent scope outside the task definition.
6. Ask the user to confirm whether this is the intended task.
7. Do not implement or modify files until the user confirms the matched task.
8. After confirmation, proceed with the task using the SDD workflow defined in `Documentation/docs/agent.md`.

Confirmation prompt:

```md
I found this task:

`Task <number> - <title>`

Brief explanation:
[Short explanation based on the task title and acceptance criteria.]

Is this the task you want me to run?
```

Flow:
User request with `/run-task <task-number>`
   ->
AI reads `Documentation/tasks/task.md`
   ->
AI finds the matching task
   ->
AI explains the task briefly
   ->
AI asks for confirmation
   ->
User confirms
   ->
AI proceeds with the task through the SDD workflow
