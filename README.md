# Raven.Assure
Configurable Raven.Backup CLI and API wrapper for [RavenDB](https://ravendb.net/).

Includes CLI tool (assure) with configurable settings and an API wrapper for Raven.Backup (Raven.Assure).

# Configs

Copy `configs/default.json` and tweak to your liking.

#### Backup location


    "Out": {
      "To": "C:\\your\\backup-folder"
    }

    "In": {
      "From": "C:\\your\\backup-folder"
    }

<div class="alert alert-warning">
`Out.To` and `In.From` must be a full file path.
</div>

(&cross;) `test.qa.raven.bak`
<br />
(&check;) `C:\\temp\\test.qa.raven.bak`

# CLI (`> assure in/out ...`)

    > assure [out|in] [config-name]

## Backup example

    > assure out test.qa

# API

- [ ] Add fluentish API docs
