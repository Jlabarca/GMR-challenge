# GMR-challenge
Unity C# Candidate Programming Challenge
Unity version: 2019.4.11f1 

## Build
[Standalone Desktop build (Windows)](https://drive.google.com/file/d/1n1FqiPg5faDpcKqmmggN6dLp9XM7Y1Au/view?usp=sharing)

## Usage
Basic unity game showing a data table reading `GMR Challenge_Data\StreamingAssets\JsonChallenge.json` content
which may be anything following this format:

```json
{
   "Title":"",
   "ColumnHeaders":[
      "column1"
   ],
   "Data":[
      {
         "column1":1
      }
   ]
}
```
- Data values can be string, boolean or number
- If JsonChallenge.json is modified the game automatically refresh the table content

## Dependencies
- NewtonSoft [Json.NET](https://www.newtonsoft.com/json) library, dll included in `Assets/Plugins` folder
