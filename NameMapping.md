# Name Map

**OData2Poco** can do explicit mapping of the names coming from
the OData feed. This is done using a JSON file and specifying
`--name-map path/to/name-map.json` on the command line.

For both types of mapping, if an explicit map is not
found, then the program reverts to the setting of `--case`.
Conversely, if a mapping is found, then the --case is ignored
for that class or property.

## Class Name Map

The class name map is a simple 1:1 match based on the original
name. The match is not case sensitive, and typical map would
look like:

```json
{
    "ClassNameMap": [
        {
            "OldName": "account",
            "NewName": "MoreSpecificAccount"
        }
    ]
}
```

## Property Name Map

The property name map has an extra layer of naming
the original class name or a special value of "ALL".
The "ALL" map also supports regex matching and the
regex matching is triggered by a leading `^` in the
OldName.

```json
{
    "PropertyNameMap": {
        "account": [
            {
                "OldName": "last_name",
                "NewName": "Surname",
            }
        ],
        "ALL": [
            {
                "OldName": "cr9f6_costapprover",
                "NewName": "CostApprover"
            },
            {
                "OldName": "^.*approver$",
                "NewName": "Approver"
            }
        ]
    }
}
```

When mapping a property, first a match is attempted
on the properties with a specific class name (account
in this example). Then a check is made in the ALL
entries but with a case-insensitive exact match to
the OldName. Lastly, an attempt is made to match the
ALL entries using a regex if the OldName starts
with a `^`.

### Complete File

The complete file would look like:

```json
{
    "ClassNameMap": [
        {
            "OldName": "account",
            "NewName": "MoreSpecificAccount"
        }
    ],
    "PropertyNameMap": {
        "account": [
            {
                "OldName": "last_name",
                "NewName": "Surname",
            }
        ],
        "ALL": [
            {
                "OldName": "cx6f4_costapprover",
                "NewName": "CostApprover"
            },
            {
                "OldName": "^.*approver$",
                "NewName": "Approver"
            }
        ]
    }
}

```
