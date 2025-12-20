# Kanji Handwriting Helper

This app shows you handwritten kanji from text input

# !!Generate db before compiling!! (/Assets/expression_rus_eng.db)

Please generate JMDICT SQLite db file from [here](https://github.com/odrevet/edict_database) using the next configuration

```bash
dart src/to_sql_expression.dart --langs "eng,rus"
```