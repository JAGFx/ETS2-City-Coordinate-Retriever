# ETS2 Log to Coordinates ##

## Note

This is a fork of [this project](https://github.com/Koenvh1/ETS2-City-Coordinate-Retriever): **Koenvh1/ETS2-City-Coordinate-Retriever**

## About

This project allow you to do:

- Retrieve the in game coordinate
- Retrieve the companies placed in each cities
- Generate JSON file with this information

## What is it ?

TODO

### Supported game and / or maps mods

Currently, only ETS2 and ATS vanilla maps

## Requirements

TODO

## Installation

TODO

## How to use ?

### Production

You can get or import the json files from the `dist/` folder. See **Dist files** section to know more.

### Developement

To generate the JSON files, you need to follow this step:

- Get or extract the cities files for ETS ou ATS (See **Raw data** section)
- Run the `coradinateRetriever` exe (See **Projects** section)
- Run the `jsonGenerator` exe (See **Projects** section)
- Use the JSON files (See **Dist files** section)

Please read the section corresponding of each step.

### Raw data

There are placed in `raw/` directory

TODO

### Projects

There are placed in `projects/` directory

Two project for two things: 
- `coradinateRetriever`: Generate a file with the cities coordinates. (Use the `goto` command from the ETS2/ATS game)
- `jsonGenerator`: Generate json files with the concatened data of cities list, companies list and cities coordinates.

### Dist files

There are placed in `dist/` directory

**A city example data**

````json
{
  "felixstowe": {
    "gameName": "felixstowe",
    "realName": "Felixstowe",
    "country": "uk",
    "companies": {
      "tree_et": {
        "gameName": "tree_et",
        "realName": "Tree-ET"
      },
      "lkwlog": {
        "gameName": "lkwlog",
        "realName": "LkwLog GmbH"
      },
      "tradeaux": {
        "gameName": "tradeaux",
        "realName": "Tradeaux"
      }
    },
    "x": "-31664.6",
    "y": "101.398",
    "z": "-13837.2"
  }
}
````

**A company example data**

````json
{
  "spinelli": {
    "gameName": "spinelli",
    "realName": "Spinelli"
  }
}
````


## Licence

Under the [MIT License](LICENSE)

© JAGFx - hey@emmanuel-smith.me
