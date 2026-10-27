# `docs/onboarding-work/v2-benchmark.json`

## Purpose

JSON data with top-level keys `lines`, `words`, `top_level_sections`, `h1_total`, `concept_count`, `fenced_blocks`, `mermaid_diagrams`, `concepts`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../onboarding-work/v2-benchmark.json). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 703 lines and 24,995 bytes. Source hash: `333807fd67111aac60b41146784914ab6e08af3c5f5edbde03f877e859c18651`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
{
  "lines": 4018,
  "words": 57697,
  "top_level_sections": 15,
  "h1_total": 18,
  "concept_count": 35,
  "fenced_blocks": 48,
  "mermaid_diagrams": 20,
  "concepts": [
    {
      "number": "6.1",
      "title": "Clean Architecture, dependency inversion and the composition root",
      "words": 1532,
      "lines": 133,
      "headings": [
        "### What it means",
        "### The problem it solves",
        "### How it works",
        "### How THIS project implements it",
        "### Execution example",
        "### Why this project needs it",
        "### Alternatives",
        "### Before you modify it",
        "### Deeper dive: layers, coupling, and the two directions of an arrow",
        "#### Start from zero: what is a \"dependency\" in code?",
        "#### What would this project look like without it?",
        "#### What this project does instead (real code)",
        "#### Two different arrows - compile time vs run time",
        "#### How the compiler and the tests enforce the rule",
        "#### Advantages",
        "#### Disadvantages (the ADR's own list, confirmed in the code)",
        "#### Alternatives",
        "#### What if we removed it here?",
        "#### Beginner traps"
      ]
    },
    {
      "number": "6.2",
      "title": "Dependency Injection: lifetimes and scanning",
      "words": 1871,
      "lines": 140,
      "headings": [
        "### What it means",
        "### The problem it solves",
        "### How it works (mechanism)",
        "### How THIS project implements it",
        "### Execution example \u2014 what happens on `dispatcher.SendAsync<CreateCentreCommand, CreateCentreResult>`",
        "### Why this project needs it",
        "### Alternatives",
        "### Before you modify it",
        "### Deeper dive: what the container really does",
        "#### The idea in plain language",
        "#### The problem, with code (hypothetical vs real)",
        "#### Registration and resolution, step by step",
        "#### Lifetimes with an analogy",
        "#### What happens when something goes wrong?",
        "#### Why constructor injection (and not setters or fields)?",
        "#### Advantages",
        "#### Disadvantages",
        "#### Alternatives",
        "#### What if we removed DI from this project?"
      ]
    },
    {
      "number": "6.3",
      "title": "CQRS and the hand-written Dispatcher",
      "words": 1443,
      "lines": 123,
      "headings": [
        "### What it means",
        "### The problem it solves",
        "### How THIS project implements it",
        "### Execution example (command, success) \u2014 `seed` creating \"Nile Tutoring Centre\"",
        "### Why this project needs it",
        "### Alternatives",
        "### Before you modify it",
        "### Deeper dive: commands, queries, mediators and pipelines",
        "#### Start from zero",
        "#### How far does this repo go? (be precise)",
        "#### What is a mediator/dispatcher, and why?",
        "#### How `Dispatcher` finds the handler (the generic-type trick)",
        "#### The pipeline, as a picture",
        "#### Advantages",
        "#### Disadvantages",
        "#### Alternatives",
        "#### What the dispatcher does **not** do (yet)",
        "#### What if we removed it here?"
      ]
    },
    {
```

The excerpt stops at line 90; use the original file for the remaining 613 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
