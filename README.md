# Kanji Handwriting Helper

This app shows you handwritten kanji from text input

# !!Place db in `/Assets/expression_rus_eng.db` before compiling!! 

You can generate JMDICT SQLite db file in two ways:
- From [here](https://github.com/odrevet/edict_database) using the this configuration:
```bash
dart src/to_sql_expression.dart --langs "eng,rus"
```

- Or by launching Windows executable as it copies db file to the same folder