using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;
using System.Globalization;



public class ApiCall : MonoBehaviour
{
    private string _basePokeURL = "https://pokeapi.co/api/v2/pokemon/";
    public TextMeshProUGUI PokemonNameAndId;
    public TextMeshProUGUI PokemonHeight;
    public TextMeshProUGUI PokemonWeight;
    public TextMeshProUGUI PokeType;
    public RawImage PokeSprite;
    public TMP_InputField PokemonNameToSearch;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Buscar()
    {
        if (PokemonNameToSearch.text != null)
        {
            StartCoroutine(GetPokemonData());
        }
    }

    IEnumerator GetPokemonData()
    {
        string urlToSearch = _basePokeURL + PokemonNameToSearch.text.ToLower();
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(urlToSearch);

        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(pokeInfoRequest.error);
            yield break;
        }

        Pokemon data = JsonUtility.FromJson<Pokemon>(pokeInfoRequest.downloadHandler.text);
        //Debug.Log(data.name + " " + data.id);

        PokemonNameAndId.text = "#"+ data.id +" " + data.name;
        PokemonHeight.text = data.height + " dm";
        PokemonWeight.text = data.weight + " hg";
        //Debug.Log(data.types[0].type.name);
        if (data.types.Length == 2)
        {
            PokeType.text = data.types[0].type.name + "-" + data.types[1].type.name;
        }
        else
        {
            PokeType.text = data.types[0].type.name;
        }

        string spriteURL = data.sprites.front_default;
        yield return LoadSprite(spriteURL);

    }

    IEnumerator LoadSprite(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else if (request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else PokeSprite.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

    }
    
}

[Serializable]
public class Pokemon
{
    public long id;
    public string name;
    public long weight;
    public long height;
    public Sprites sprites;
    public TypeElement[] types;
}

[Serializable]
public partial class Sprites
{
    public string front_default;
}
[Serializable]
public partial class TypeElement
{
    public TypeType type;
}
[Serializable]
public partial class TypeType
{
    public string name;

}
