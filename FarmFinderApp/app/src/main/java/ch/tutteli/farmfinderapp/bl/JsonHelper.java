package ch.tutteli.farmfinderapp.bl;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

public class JsonHelper {
    public static JSONArray InputStreamToJsonArray(InputStream inputStream) throws IOException, JSONException {
        BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream));
        StringBuilder sb = new StringBuilder();
        String line = null;
        while ((line = reader.readLine()) != null) {
            sb.append(line + "\n");
        }
        String s = sb.toString();
        if (!s.equals("")) {
            return new JSONArray(sb.toString());
        } else {
            return null;
        }
    }
}