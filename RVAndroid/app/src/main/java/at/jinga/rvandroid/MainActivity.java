package at.jinga.rvandroid;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.EditText;

import java.io.IOException;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;

public class MainActivity extends AppCompatActivity {
    private Socket server;
    private PrintWriter out;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    @Override
    protected void onStop() {
        super.onStop();

        if (server != null && server.isConnected()) {
            out.println("disconnected");
        }
    }

    public void connect(View view) {
        new Thread()
        {
            public void run() {
                EditText ip = (EditText) findViewById(R.id.ip);

                try {
                    server = new Socket(ip.getText().toString(), 3131);

                    out = new PrintWriter(server.getOutputStream(), true);
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }.start();
    }

    public void send(View view) {
        if (server != null && server.isConnected()) {
            EditText command = (EditText) findViewById(R.id.command);

            out.println(command.getText().toString());
        }
    }
}
