import json


def make(language):
    global current_hidden_id
    global current_id
    file = open("Data.json", "r")
    options = json.load(file)
    file.close()

    translation = {}
    base = {"Mods": {"AdvancedWorldGen": translation}}

    translation["Conflict"] = {}
    conflicts = translation["Conflict"]
    make_trans(conflicts, language, options, translation)

    file = open(language + ".json", "w")
    json.dump(base, file, indent=4, sort_keys=True)
    file.close()

    json_text = {}
    current_id = 1
    current_hidden_id = 101
    make_options(json_text, options)

    file = open("Options.json", "w")
    json.dump(json_text, file, indent=4)
    file.close()


def make_options(json_text, options):
    global current_hidden_id
    global current_id
    for key in options:
        option = options[key]
        o = {}
        if option["hidden"]:
            o["Hidden"] = True
            o["id"] = current_hidden_id
            current_hidden_id += 1
        else:
            o["Hidden"] = False
            o["id"] = current_id
            current_id += 1
        o["Conflicts"] = list(option["conflicts"].keys())
        o["Name"] = key
        children = []
        make_options(children, option["children"])
        o["Children"] = children
        if isinstance(json_text, list):
            json_text.append(o)
        else:
            json_text[key] = o


def make_trans(conflicts, language, options, translation, prefix=""):
    for key in options:
        option = options[key]

        name = option["displayed_name"]
        if type(name) is dict:
            name = name[language]

        description = option["description"]
        if type(description) is dict:
            description = description[language]

        usual_translation = {"$parentVal": name, "Description": description}
        translation[key] = usual_translation
        conflicts_dict = option["conflicts"]
        internal_conflicts = {}
        conflicts[key] = internal_conflicts
        for conflict_key in conflicts_dict:
            description = conflicts_dict[conflict_key]
            if type(description) is dict:
                description = description[language]
            internal_conflicts[conflict_key] = description

        if prefix == "":
            make_trans(internal_conflicts, language, option["children"], usual_translation, key)
        else:
            make_trans(internal_conflicts, language, option["children"], usual_translation, prefix + "." + key)


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
