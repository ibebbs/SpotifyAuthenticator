# Spotify Authenticator

A .Net Core console application for retrieving access and refresh tokens from Spotify.

# Background

In order to any vaguely interesting endpoints in [Spotify's Web API](https://developer.spotify.com/web-api/) you need two things:

1. A client application
2. Authorization from the user for the client application to access their resources

## Creating a client application

TBC

## Authorizing the use of user resources

That is where this utility comes in. It allows you to easily retreive a 'refreshable' access token from Spotify.


# Usage

## Authorization

The `authorize` command is used to initiate a ['Authorization Code' ](https://beta.developer.spotify.com/documentation/general/guides/authorization-guide/#authorization-code-flow) flow. This command requires both your client id and client secret and displays a prompt to the user (using the supplied browser) authorizing the client app's access the users resources.

`
dotnet SpotifyAuthenticator.dll -c <clientId> -k <clientSecret> -s <scope> -o <output token as json> -b <path to browser for authentication>
`

e.g.

`
dotnet SpotifyAuthenticator.dll authorize -c ############## -k ####################### -s UserReadPrivate -s UserReadPlaybackState -s UserModifyPlaybackState -o "c:\Temp\Token.json" -b "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
`

## Refresh Token

TBC

# Attribution

This project uses [Jonas Dellinger's](https://github.com/JohnnyCrazy) awesome [SpotifyAPI-NET](https://github.com/JohnnyCrazy/SpotifyAPI-NET) project. A custom build of this library was made by yours truely to support .NET Core an is currently awaiting [PR approval](https://github.com/JohnnyCrazy/SpotifyAPI-NET/pull/237).