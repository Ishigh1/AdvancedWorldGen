import json


def make(language):
    file = open("Data.json", "r")
    options = json.load(file)
    file.close()

    translation = {}
    base = {"Mods": {"AdvancedWorldGen": translation}}

    make_trans(language, options, translation)

    file = open(language + ".json", "w")
    json.dump(base, file, indent=4, sort_keys=True)
    file.close()

    json_text = {}
    make_options(json_text, options)

    file = open("Options.json", "w")
    json.dump(json_text, file, indent=4)
    file.close()


def make_options(json_text, options):
    for key in options:
        option = options[key]
        o = {
            "Hidden": option["hidden"],
            "Conflicts": list(option["conflicts"].keys()),
            "Name": key
        }
        children = []
        make_options(children, option["children"])
        o["Children"] = children
        if isinstance(json_text, list):
            json_text.append(o)
        else:
            json_text[key] = o


def make_trans(language, options, translation, prefix=""):
    for key in options:
        option = options[key]

        name = option["displayed_name"]
        if type(name) is dict:
            name = name[language]

        description = option["description"]
        if type(description) is dict:
            description = description[language]

        conflicts = {}
        usual_translation = {"$parentVal": name, "Description": description, "Conflicts": conflicts}
        translation[key] = usual_translation
        conflicts_dict = option["conflicts"]
        for conflict_key in conflicts_dict:
            description = conflicts_dict[conflict_key]
            if type(description) is dict:
                description = description[language]

            split_key = conflict_key.split(".")
            internal_conflicts = conflicts
            for part_key in split_key:
                if part_key not in internal_conflicts:
                    internal_conflicts[part_key] = {}
                internal_conflicts = internal_conflicts[part_key]
            internal_conflicts["$parentVal"] = description

        if prefix == "":
            make_trans(language, option["children"], usual_translation, key)
        else:
            make_trans(language, option["children"], usual_translation, prefix + "." + key)


def translate(initial, language):
    if type(initial) is not dict:
        result = {"en-US": initial}
    elif initial.__contains__(language):
        return initial
    else:
        result = initial

    print("english value : " + result["en-US"])
    result[language] = input("translation : ")

    return result
