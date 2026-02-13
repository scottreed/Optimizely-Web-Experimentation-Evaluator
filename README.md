# Optimizely Web Experimentation Evaluator

This project is a POC around evaulating Optimizely Web Experimentation via the REST API using the bearer token for a web experimentaion user. 

Web experimentation Counts its monthly active users based upon a script being just rendered into the page to evaluate if an experiment needs to be run. This means that whenever you render a web experimentation snippet, even if there is no need to run it, it will evaluate and for an unique user count as an MAU.

This can increase costs as you're counting users that don't need to run this is especially important with multi-site where you might not be running experiments on specific sections of pages or subsites. 

## Project Overview

This is a standard project with the following

- .NET 8.0
- ASP.NET Core 8.0
- Optimizely SDK
- Minimal API
- In Memory Cache

## Features

The project has the following features

- Return a list of experiments from the web experimentation REST API based upon the past in status flags. 
- Filtering the list of experiments based upon multiple path criteria (e.g., a sub-path or the route of the website). 
- A Boolean in the `ExperimentFilterService` service to allow returning experiments if they have no URL matching criteria. 
- In-memory caching of experiments so they don't need to be constantly re-evaluated with a 15-minute window. 
- A minimal API that matches the webhook JSON Post format. This webhook can be configured in the settings for a project and notifies you when a project has a change. This minimal API will clear the cache based upon the project ID. 

## Setup

- Clone the project
- Update the `appsettings.json` file with the bearer token
- Run the project
- In the UI set your project ID and the root URL and subpath.
- Check the checkbox to match any non url matching experiments


