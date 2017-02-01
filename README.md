# Raven.Assure
Configurable Raven.Backup CLI and API wrapper for [RavenDB](https://ravendb.net/).

Includes CLI tool (assure) with configurable settings and an API wrapper for Raven.Backup (Raven.Assure).

# Configs

Copy `configs/default.json` and tweak to your liking.

#### Backup location

    "Out": {
      "To": {
        "FilePath": "C:\\your\\backup-folder"
      }
    }

    "In": {
      "From": {
        "FilePath": "C:\\your\\backup-folder"
      }
    }

**Warning**: `Out.To.FilePath` and `In.From.FilePath` must be a full file path.

(&cross;) `test.qa.raven.bak`
<br />
(&check;) `C:\\temp\\test.qa.raven.bak`

#### Incremental

    "Out": {
      "Incremental": true
    }

Incremental backups require [particular settings](https://ravendb.net/docs/article-page/3.0/csharp/server/administration/backup-and-restore#using-the-raven.backup-utility) in the RavenDB [server configuration](https://ravendb.net/docs/article-page/3.0/csharp/server/configuration/configuration-options).

# CLI (`> assure in/out ...`)

    > assure [out|in] [config-name]

## Backup example

    > assure out test.qa

# API

- [ ] Add fluentish API docs
