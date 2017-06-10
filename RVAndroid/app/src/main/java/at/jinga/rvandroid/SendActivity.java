package at.jinga.rvandroid;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class SendActivity extends AppCompatActivity {
    private Socket server;
    private PrintWriter out;
    private BufferedReader in;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_send);

        server = ConnectionManager.server;

        try {
            out = new PrintWriter(server.getOutputStream(), true);
            in = new BufferedReader(new InputStreamReader(server.getInputStream()));
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    @Override
    protected void onStop() {
        super.onStop();

        disconnect(null);
    }

    protected void send(View view) {
        if (server != null && server.isConnected()) {
            EditText command = (EditText) findViewById(R.id.command);

            out.println(command.getText().toString());
        }
    }

    public void disconnect(View view) {
        if (server != null && server.isConnected()) {
            out.println("disconnect");
            out.close();

            try {
                server.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
            server = null;

            startActivity(new Intent(SendActivity.this, MainActivity.class));
        }
    }
}
